#!/usr/bin/env node

const { spawn } = require('child_process');
const { writeFileSync } = require('fs');

const oldSchema = process.argv[2]
const newSchema = process.argv[3]
const allowBreakingChanges = process.argv[4] === 'true'
const reportPath = process.argv[5]

runProcess('graphql-inspector', ['diff', oldSchema, newSchema])
    .then(async ({ code, output }) => {
        const report = resolveReport(output)
        writeFileSync(reportPath, report)
        console.log('Report')
        console.log(report)

        // don't care about errors if breaking changes are allowed
        if (allowBreakingChanges)
            return;

        if (code > 0 || output.includes('✖'))
            process.exit(code || 1)
        else
            process.exit(0)
    })

function runProcess(cmd, args) {
    return new Promise(resolve => {
        const output = []
        const childProcess = spawn(cmd, args);

        childProcess.stdout.on('data', data => output.push(data));
        childProcess.stderr.on('data', data => output.push(data));

        childProcess.on('exit', code => resolve({ code, output: Buffer.concat(output).toString() }));
    });
}

function resolveReport(report) {
    if (report.includes('No changes detected'))
        return `### ✅ Schema ${newSchema} has no changes`

    if (report.includes('GraphQLError: Syntax Error') || report.includes('Unable to find any GraphQL type definitions for the following pointers'))
        return `### ❌ Schema ${newSchema} is broken and can't be validated`

    return `### ⚠️ Schema ${newSchema} validation failed:\n\n${formatReport(report)}`;
}

function formatReport(report) {
    return report
        .replaceAll('✖', '❌')
        .replaceAll('✔', '✅')
        .replaceAll('[log]', '')
        .replaceAll('[error]','####')
        .trim();
}
